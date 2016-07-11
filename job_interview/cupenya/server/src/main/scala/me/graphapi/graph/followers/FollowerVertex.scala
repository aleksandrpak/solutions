package me.graphapi.graph.followers

import akka.NotUsed
import akka.stream.ActorMaterializer
import akka.stream.scaladsl.Source
import akka.util.Timeout
import me.graphapi.graph.Vertex
import me.graphapi.util.DatabaseService

import scala.concurrent.Future

class FollowerVertex(databaseService: DatabaseService,
                     implicit val actorMaterializer: ActorMaterializer,
                     implicit val operationTimeout: Timeout) extends Vertex {

  def hasEdgeTo(to: Int): Future[Boolean] = databaseService.hasFollower(id, to)

  def getEdges: Source[Int, NotUsed] = databaseService.getFollowers(id).map(_.id.get)
}
