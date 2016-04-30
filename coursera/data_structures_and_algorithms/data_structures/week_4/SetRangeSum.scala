import scala.io.Source

case class Node[T <: Ordered[T]](var key: Int, var sum: Int, var height: Int, var parent: Option[Node[T]], var left: Option[Node[T]], var right: Option[Node[T]])

class SplayTree[T <: Ordered[T]] {

  var root: Option[Node[T]] = None

  private def rotateLeft(node: Node[T]) {
    val right = node.right
    node.right = right.flatMap { r =>
      r.left.foreach { _.parent = Some(node) }
      r.parent = node.parent
      r.left
    }

    updateParent(node, right)

    right.foreach { _.left = Some(node) }
    node.parent = right
  }

  private def rotateRight(node: Node[T]) {
    val left = node.left
    node.left = left.flatMap { l =>
      l.right.foreach { _.parent = Some(node) }
      l.parent = node.parent
      l.right
    }

    updateParent(node, left)

    left.foreach { _.right = Some(node) }
    node.parent = left
  }

  private def updateParent(node: Node[T], child: Option[Node[T]]) {
    node.parent match {
      case Some(parent) if node == parent.left => parent.left = child
      case Some(parent) => parent.right = child
      case None => root = child
    }
  }
}

object SetRangeSum extends App {

  val lines = Source.stdin.getLines
  val n = lines.next.toInt

  lines.foreach { line =>
    println(line)
  }

  println(n)
}
