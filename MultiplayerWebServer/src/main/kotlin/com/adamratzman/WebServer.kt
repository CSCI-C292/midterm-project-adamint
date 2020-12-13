package com.adamratzman

import org.eclipse.jetty.websocket.api.Session
import org.eclipse.jetty.websocket.api.annotations.OnWebSocketClose
import org.eclipse.jetty.websocket.api.annotations.OnWebSocketConnect
import org.eclipse.jetty.websocket.api.annotations.OnWebSocketMessage
import org.eclipse.jetty.websocket.api.annotations.WebSocket
import spark.Spark.*
import java.io.IOException
import java.util.concurrent.ConcurrentHashMap
import java.util.concurrent.CopyOnWriteArrayList
import kotlin.math.sin
import kotlin.random.Random


fun main() {
    port(1337)
    webSocket("/connect", FlappySocket::class.java)
    init()
}

@WebSocket
class FlappySocket {
    // Store sessions if you want to, for example, broadcast a message to all users
    private val sessions: CopyOnWriteArrayList<Session> = CopyOnWriteArrayList()
    val games: ConcurrentHashMap<Session, Game> = ConcurrentHashMap()

    @OnWebSocketConnect
    fun connected(session: Session) {
        sessions.add(session)
    }

    @OnWebSocketClose
    fun closed(session: Session, statusCode: Int, reason: String?) {
        if (games.containsKey(session)) {
            val game = games[session]!!
            game.sessions.remove(session)
            game.usernamesSessions[session]?.forEach { username -> game.onPlayerDeath(username) }
        }
        sessions.remove(session)
    }

    @OnWebSocketMessage
    @Throws(IOException::class)
    fun message(session: Session, message: String) {
        try {
            val split = message.split(" ")
            val receivedMessage =
                ReceivedMessage(split[0], if (split.size == 1) listOf() else split.subList(1, split.size))
            val game = games[session]
            when (Actions.values().find { it.sendString == receivedMessage.action }) {
                Actions.CONNECT_TO_GAME -> {
                    if (game == null && receivedMessage.arguments.isNotEmpty()) {
                        val gameId = receivedMessage.arguments.joinToString(" ")
                        val foundGame = games.values.find { it.id == gameId }
                        if (foundGame != null) {
                            foundGame.sessions.add(session)
                            foundGame.usernames.forEach { existingUsername ->
                                session.remote.sendString("${Actions.ADD_PLAYER} $existingUsername")
                            }
                            games[session] = foundGame
                        } else {
                            val newGame = Game(gameId, mutableSetOf(session), this)
                            games[session] = newGame
                            session.remote.sendString(Actions.SUCCESS_CONNECTED_TO_GAME.sendString)
                        }
                    }
                }
                Actions.ADD_PLAYER -> {
                    println("add player")
                    println(game)
                    println(receivedMessage.arguments)
                    if (game != null && receivedMessage.arguments.isNotEmpty()) {
                        game.addPlayer(receivedMessage.arguments.joinToString(" "), session)
                    }
                }
                Actions.PLAYER_DEATH -> {
                    val player = receivedMessage.arguments.joinToString(" ")
                    if (game != null && receivedMessage.arguments.isNotEmpty() && player in game.usernames) {
                        game.onPlayerDeath(player)
                    }
                }
                Actions.PLAYER_PIPE_PASSED -> {
                    val player = receivedMessage.arguments.joinToString(" ")
                    if (game != null && receivedMessage.arguments.isNotEmpty() && player in game.usernames) {
                        game.onPipePassed(player)
                    }
                }
                Actions.START_GAME -> {
                    if (game != null && !game.started) {
                        game.startGame()
                    }
                }
                Actions.PLAYER_FLAP -> {
                    val player = receivedMessage.arguments.joinToString(" ")
                    if (game != null && receivedMessage.arguments.isNotEmpty() && player in game.usernames) {
                        game.onPlayerFlap(player, session)
                    }
                }
                Actions.SPAWN_PIPE_REQUEST -> {
                    val x = receivedMessage.arguments.getOrNull(0)?.toFloatOrNull()
                    if (x != null && game != null) {
                        game.onPipeSpawnRequest(x, session)
                    }
                }
            }

            println("Got: $message")
        }catch (e:Exception){e.printStackTrace()}
    }

}

data class ReceivedMessage(val action: String, val arguments: List<String>)

data class Game(
    val id: String,
    val sessions: MutableSet<Session>,
    val flappySocket: FlappySocket
) {
    val usernames: MutableList<String> = mutableListOf()
    val usernamesSessions: MutableMap<Session, List<String>> = mutableMapOf()
    var started: Boolean = false

    lateinit var scores: MutableMap<String, Int>
    lateinit var alive: MutableMap<String, Boolean>

    lateinit var pipeSpawnRequests: MutableMap<Session, Int>
    val pipes: MutableList<Pair<Float, Float>> = mutableListOf()

    fun onPipeSpawnRequest(x: Float, sessionRequesting: Session) {
        pipeSpawnRequests.replace(sessionRequesting, pipeSpawnRequests[sessionRequesting]!! + 1)
        val currentIndex = pipeSpawnRequests[sessionRequesting]!!
        if (currentIndex > pipes.lastIndex) {
            (pipes.size..currentIndex).forEach { _ ->
                val downHeight = (sin(Random.nextDouble(0.0, Math.PI / 2.0)) * (5.0 - 2.04)).toFloat()
                val downPipeY = downHeight - 5f
                val upPipeY = downPipeY + 7.5f

                pipes.add(downPipeY to upPipeY)
            }
        }
        val pipe = pipes[currentIndex]
        sessionRequesting.remote.sendString("${Actions.SPAWN_PIPE_REQUEST} $x ${pipe.first} ${pipe.second}")
    }

    fun addPlayer(username: String, session: Session) {
        println("adding $username")
        usernames.add(username)
        sessions.add(session)
        usernamesSessions.putIfAbsent(session, mutableListOf())
        usernamesSessions.replace(session, usernamesSessions[session]!! + username)
        sendMessageToAll("${Actions.ADD_PLAYER} $username")
        println("added")
    }

    fun startGame() {
        scores = usernames.map { it to 0 }.toMap().toMutableMap()
        alive = usernames.map { it to true }.toMap().toMutableMap()
        started = true
        pipeSpawnRequests = sessions.map { it to 0 }.toMap().toMutableMap()
        sendMessageToAll(Actions.START_GAME.sendString)
    }

    fun onPipePassed(username: String) {
        scores.replace(username, scores[username]!! + 1)
        sendMessageToAll("${Actions.PLAYER_PIPE_PASSED} $username")
    }

    fun onPlayerDeath(username: String) {
        alive[username] = false
        sendMessageToAll("${Actions.PLAYER_DEATH} $username")
        if (alive.values.none { it }) onGameOver()
    }

    fun onPlayerFlap(username: String, session: Session) {
        sessions.filter { it != session }.forEach { it.remote.sendString("${Actions.PLAYER_FLAP} $username");println("sent player flap") }
    }

    fun onGameOver() {
        sendMessageToAll(Actions.GAME_OVER.sendString)

        sessions.forEach { session ->
            flappySocket.games.remove(session)
            session.disconnect()
        }
    }

    fun sendMessageToAll(message: String) = sessions.forEach { it.remote.sendString(message) }
}

enum class Actions(val sendString: String) {
    ADD_PLAYER("add-player"),
    START_GAME("start-game"),
    PLAYER_PIPE_PASSED("pipe-passed"),
    PLAYER_DEATH("player-death"),
    GAME_OVER("game-over"),
    CONNECT_TO_GAME("connect-to-game"),
    SUCCESS_CONNECTED_TO_GAME("success-connected-to-game"),
    PLAYER_FLAP("player-flap"),
    SPAWN_PIPE_REQUEST("spawn-pipe");
    ;

    override fun toString() = sendString
}