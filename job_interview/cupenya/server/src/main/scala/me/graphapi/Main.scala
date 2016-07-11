package me.graphapi

import akka.actor.ActorSystem
import akka.event.Logging
import akka.http.scaladsl.Http
import akka.stream.ActorMaterializer
import akka.util.Timeout
import me.graphapi.http.HttpService
import me.graphapi.services.{FollowersGraphService, PersonsService}
import me.graphapi.util.{Config, DatabaseService}

import scala.concurrent.duration._

object Main extends App with Config {
  implicit val system = ActorSystem()
  implicit val executionContext = system.dispatcher
  implicit val log = Logging(system, getClass)
  implicit val materializer = ActorMaterializer()
  implicit val timeout = Timeout(operationsTimeout.millis)

  val databaseService = new DatabaseService()
  val followersGraphService = new FollowersGraphService(shortestPathMaxDepth, databaseService, system, materializer)
  val personsService = new PersonsService(databaseService, followersGraphService)

  val httpService = new HttpService(personsService)

  Http().bindAndHandle(httpService.routes, httpHost, httpPort)
}
