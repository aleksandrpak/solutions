import java.io.IOException;

import org.apache.hadoop.conf.Configuration;

import org.apache.hadoop.hbase.HBaseConfiguration;
import org.apache.hadoop.hbase.HColumnDescriptor;
import org.apache.hadoop.hbase.HTableDescriptor;

import org.apache.hadoop.hbase.TableName;

import org.apache.hadoop.hbase.client.HBaseAdmin;
import org.apache.hadoop.hbase.client.HTable;
import org.apache.hadoop.hbase.client.Put;
import org.apache.hadoop.hbase.client.Result;
import org.apache.hadoop.hbase.client.ResultScanner;
import org.apache.hadoop.hbase.client.Scan;

import org.apache.hadoop.hbase.util.Bytes;

public class SuperTable{

    public static void main(String[] args) throws IOException {
        // Create
        Configuration con = HBaseConfiguration.create();
        HBaseAdmin admin = new HBaseAdmin(con);
        HTableDescriptor tableDescriptor = new HTableDescriptor(TableName.valueOf("powers"));

        tableDescriptor.addFamily(new HColumnDescriptor("personal"));
        tableDescriptor.addFamily(new HColumnDescriptor("professional"));

        admin.createTable(tableDescriptor);

        // Insert
        Configuration config = HBaseConfiguration.create();
        HTable hTable = new HTable(config, "powers");

        Put p1 = new Put(Bytes.toBytes("row1"));
        p1.add(Bytes.toBytes("personal"), Bytes.toBytes("hero"), Bytes.toBytes("superman"));
        p1.add(Bytes.toBytes("personal"), Bytes.toBytes("power"), Bytes.toBytes("strength"));
        p1.add(Bytes.toBytes("professional"), Bytes.toBytes("name"), Bytes.toBytes("clark"));
        p1.add(Bytes.toBytes("professional"), Bytes.toBytes("xp"), Bytes.toBytes("100"));

        Put p2 = new Put(Bytes.toBytes("row2"));
        p2.add(Bytes.toBytes("personal"), Bytes.toBytes("hero"), Bytes.toBytes("batman"));
        p2.add(Bytes.toBytes("personal"), Bytes.toBytes("power"), Bytes.toBytes("money"));
        p2.add(Bytes.toBytes("professional"), Bytes.toBytes("name"), Bytes.toBytes("bruce"));
        p2.add(Bytes.toBytes("professional"), Bytes.toBytes("xp"), Bytes.toBytes("50"));

        Put p3 = new Put(Bytes.toBytes("row3"));
        p3.add(Bytes.toBytes("personal"), Bytes.toBytes("hero"), Bytes.toBytes("wolverine"));
        p3.add(Bytes.toBytes("personal"), Bytes.toBytes("power"), Bytes.toBytes("healing"));
        p3.add(Bytes.toBytes("professional"), Bytes.toBytes("name"), Bytes.toBytes("logan"));
        p3.add(Bytes.toBytes("professional"), Bytes.toBytes("xp"), Bytes.toBytes("75"));

        hTable.put(p1);
        hTable.put(p2);
        hTable.put(p3);

        // Scan
        Scan scan = new Scan();

        scan.addColumn(Bytes.toBytes("personal"), Bytes.toBytes("hero"));

        ResultScanner scanner = hTable.getScanner(scan);
        for (Result result = scanner.next(); result != null; result = scanner.next())
            System.out.println(result);

        scanner.close();
        hTable.close();
    }
}
