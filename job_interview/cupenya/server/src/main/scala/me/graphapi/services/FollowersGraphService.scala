package me.graphapi.services

import akka.actor.{ActorSystem, Props}
import akka.pattern._
import akka.stream.ActorMaterializer
import akka.util.Timeout
import me.graphapi.graph.followers.FollowersGraph
import me.graphapi.graph.{FindShortestPath, ShortestPath}
import me.graphapi.util.DatabaseService

import scala.concurrent.{ExecutionContext, Future}

class FollowersGraphService(shortestPathMaxDepth: Int, databaseService: DatabaseService,
                            system: ActorSystem, materializer: ActorMaterializer)
                           (implicit val executionContext: ExecutionContext,
                            implicit val timeout: Timeout) {

  private val graph = system.actorOf(Props(classOf[FollowersGraph], databaseService, materializer, timeout), "FollowersGraph")

  def shortestPath(from: Int, to: Int): Future[List[Int]] = {
    val answer = graph ? FindShortestPath(from, to, shortestPathMaxDepth)

    answer.map {
      case ShortestPath(path) => path
    }
  }
}
