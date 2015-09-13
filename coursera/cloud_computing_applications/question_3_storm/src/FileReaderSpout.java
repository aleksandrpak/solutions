import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.util.Map;

import backtype.storm.spout.SpoutOutputCollector;
import backtype.storm.task.TopologyContext;
import backtype.storm.topology.IRichSpout;
import backtype.storm.topology.OutputFieldsDeclarer;
import backtype.storm.tuple.Fields;
import backtype.storm.tuple.Values;

public class FileReaderSpout implements IRichSpout {
    private SpoutOutputCollector _collector;
    private TopologyContext context;
    private BufferedReader _reader;
    private String _filename;

    public FileReaderSpout(String filename) {
        this._filename = filename;
    }

    @Override
    public void open(Map conf, TopologyContext context,
                     SpoutOutputCollector collector) {
        this._collector = collector;
        this.context = context;

        try {
            this._reader = new BufferedReader(new FileReader(_filename));
        } catch (IOException ioe) {
        }
    }

    @Override
    public void nextTuple() {
        String line = null;

        try {
            line = _reader.readLine();
        } catch (IOException e) {
        }

        if (line == null) {
            return;
        }

        _collector.emit(new Values(line));
    }

    @Override
    public void declareOutputFields(OutputFieldsDeclarer declarer) {
        declarer.declare(new Fields("word"));
    }

    @Override
    public void close() {
        try {
            _reader.close();
        } catch (IOException e) {
        }
    }

    @Override
    public void activate() {
    }

    @Override
    public void deactivate() {
    }

    @Override
    public void ack(Object msgId) {
    }

    @Override
    public void fail(Object msgId) {
    }

    @Override
    public Map<String, Object> getComponentConfiguration() {
        return null;
    }
}
