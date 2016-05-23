package me.graphapi.graph

import akka.actor.{Actor, ActorRef}

final case class IncrementExpectedResponses()

final case class ExpectedResponsesIncremented()

class ShortestPathAggregator(replyTo: ActorRef, from: ActorRef, to: Int, maxDepth: Int) extends Actor {

  private var expectedResponses = 1
  private var receivedResponses = 0

  def receive = {
    case IncrementExpectedResponses =>
      expectedResponses += 1
      sender ! ExpectedResponsesIncremented

    case PathTo(path, targetDepth) =>
      if (path.nonEmpty) {
        replyTo ! ShortestPath(path)
        context.stop(self)
      }

      receivedResponses += 1

      if (expectedResponses == receivedResponses) {
        if (targetDepth == maxDepth) {
          replyTo ! ShortestPath(List())
          context.stop(self)
        } else {
          receivedResponses = 0
          expectedResponses = 1

          from ! FindPathTo(self, List(), to, targetDepth + 1)
        }
      }
  }
}
