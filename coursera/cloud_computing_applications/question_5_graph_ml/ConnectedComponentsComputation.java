import org.apache.giraph.graph.BasicComputation;
import org.apache.giraph.edge.Edge;
import org.apache.giraph.graph.Vertex;
import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.NullWritable;

import java.io.IOException;

/**
 * Implementation of the connected component algorithm that identifies
 * connected components and assigns each vertex its "component
 * identifier" (the smallest vertex id in the component).
 */
public class ConnectedComponentsComputation extends
        BasicComputation<IntWritable, IntWritable, NullWritable, IntWritable> {
    /**
     * Propagates the smallest vertex id to all neighbors. Will always choose to
     * halt and only reactivate if a smaller id has been sent to it.
     *
     * @param vertex   Vertex
     * @param messages Iterator of messages from the previous superstep.
     * @throws IOException
     */
    @Override
    public void compute(
            Vertex<IntWritable, IntWritable, NullWritable> vertex,
            Iterable<IntWritable> messages) throws IOException {
        int currentComponent = vertex.getValue().get();

        if (getSuperstep() == 0) {
            for (Edge<IntWritable, NullWritable> edge : vertex.getEdges()) {
                int neighbor = edge.getTargetVertexId().get();
                if (neighbor < currentComponent) {
                    currentComponent = neighbor;
                }
            }
            if (currentComponent != vertex.getValue().get()) {
                vertex.setValue(new IntWritable(currentComponent));
                for (Edge<IntWritable, NullWritable> edge : vertex.getEdges()) {
                    IntWritable neighbor = edge.getTargetVertexId();
                    if (neighbor.get() > currentComponent) {
                        sendMessage(neighbor, vertex.getValue());
                    }
                }
            }
            vertex.voteToHalt();
            return;
        }

        boolean changed = false;
        for (IntWritable message : messages) {
            int candidateComponent = message.get();
            if (candidateComponent < currentComponent) {
                currentComponent = candidateComponent;
                changed = true;
            }
        }

        if (changed) {
            vertex.setValue(new IntWritable(currentComponent));
            sendMessageToAllEdges(vertex, vertex.getValue());
        }

        vertex.voteToHalt();

    }
}
