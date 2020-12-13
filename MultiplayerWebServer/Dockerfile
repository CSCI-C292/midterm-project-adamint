FROM openjdk:8-jre-slim

EXPOSE 1337

COPY ./build /server

WORKDIR /server

ENTRYPOINT ["/server/install/MultiplayerWebServer/bin/MultiplayerWebServer"]