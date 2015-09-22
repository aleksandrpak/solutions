import java.util.regex.Pattern;

import org.apache.spark.SparkConf;
import org.apache.spark.api.java.JavaPairRDD;
import org.apache.spark.api.java.JavaRDD;
import org.apache.spark.api.java.JavaSparkContext;
import org.apache.spark.api.java.function.Function;

import org.apache.spark.api.java.function.PairFunction;
import org.apache.spark.api.java.function.VoidFunction;
import org.apache.spark.mllib.clustering.KMeans;
import org.apache.spark.mllib.clustering.KMeansModel;
import org.apache.spark.mllib.linalg.Vector;
import org.apache.spark.mllib.linalg.Vectors;
import scala.Tuple2;


public final class KMeansMP {
    private static class ParsePoint implements Function<String, Vector> {
        private static final Pattern SPACE = Pattern.compile(",");

        public Vector call(String line) {
            String[] tok = SPACE.split(line);
            double[] point = new double[tok.length - 1];
            for (int i = 1; i < tok.length; ++i) {
                point[i - 1] = Double.parseDouble(tok[i]);
            }
            return Vectors.dense(point);
        }
    }

    private static class ParseTitle implements Function<String, String> {
        private static final Pattern SPACE = Pattern.compile(",");

        public String call(String line) {
            String[] tok = SPACE.split(line);
            return tok[0];
        }
    }

    private static class ClusterCars implements PairFunction<Tuple2<String, Vector>, Integer, String> {
        private KMeansModel model;

        public ClusterCars(KMeansModel model) {
            this.model = model;
        }

        public Tuple2<Integer, String> call(Tuple2<String, Vector> args) {
            String title = args._1();
            Vector point = args._2();
            int cluster = model.predict(point);
            return new Tuple2<Integer, String>(cluster, title);
        }
    }

    public static void main(String[] args) {
        if (args.length < 2) {
            System.err.println(
                    "Usage: KMeansMP <input_file> <results>");
            System.exit(1);
        }

        String inputFile = args[0];
        String results_path = args[1];
        JavaPairRDD<Integer, Iterable<String>> results;

        int k = 4;
        int iterations = 100;
        int runs = 1;
        long seed = 0;
        final KMeansModel model;

        SparkConf sparkConf = new SparkConf().setAppName("KMeans MP");
        JavaSparkContext sc = new JavaSparkContext(sparkConf);
        JavaRDD<String> lines = sc.textFile(inputFile);

        JavaRDD<Vector> points = lines.map(new ParsePoint());
        JavaRDD<String> titles = lines.map(new ParseTitle());
        model = KMeans.train(points.rdd(), k, iterations, runs, KMeans.RANDOM(), 0);
        results = titles.zip(points).mapToPair(new ClusterCars(model)).groupByKey();

        results.saveAsTextFile(results_path);

        sc.stop();
    }
}
