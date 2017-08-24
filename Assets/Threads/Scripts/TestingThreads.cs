using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingThreads : MonoBehaviour {

    private Dictionary<int, List<float>> dict;

    private int nrItems = 25;
    private int nrThreads = 4;
    private int workLoad;

    private void Awake()
    {
        dict = new Dictionary<int, List<float>>();

        workLoad = nrItems / nrThreads;

        // load initial random data
        for (int i = 0; i < nrItems; i++)
        {
            dict.Add(i+10, new List<float>());

            for (int j = 0; j < 5; j++)
            {
                dict[i+10].Add(Random.Range(0f, 1f));
            }
        }

        // print unsorted data
        foreach (KeyValuePair<int, List<float>> entry in dict)
        {
            printEntry(entry);
        }


        List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
        List<int> keys = new List<int>(dict.Keys);

        // sort data in parallel
        for (int i = 0; i < nrThreads; i++)
        {
            // pack parameters
            object param = new object[6] { i, nrThreads, workLoad, nrItems, dict, keys };
            System.Threading.ParameterizedThreadStart pts = new System.Threading.ParameterizedThreadStart(process);
            System.Threading.Thread worker = new System.Threading.Thread(pts);
            threads.Add(worker);
            worker.Start(param);
        }


        for (int i = 0; i < threads.Count; i++)
        {
            threads[i].Join();
        }

        print("main thread time: " + Time.realtimeSinceStartup);


        // print sorted data
        foreach (KeyValuePair<int, List<float>> entry in dict)
        {
            printEntry(entry);
        }


    }

    private void printEntry (KeyValuePair<int, List<float>> entry)
    {
        string buffer = "[";
        for (int i = 0; i < entry.Value.Count; i++)
        {
            buffer += entry.Value[i].ToString() + " ,";
        }
        buffer += "]";

        print(entry.Key + ": " + buffer);
    }

    private void process (object _arg)
    {
        object[] arg = new object[5];
        arg = (object[])_arg;

        int th                              = (int)arg[0];
        int nrThreads                       = (int)arg[1];
        int workLoad                        = (int)arg[2];
        int nrItems                         = (int)arg[3];
        Dictionary<int, List<float>> data   = (Dictionary<int, List<float>>)arg[4];
        List<int> keys = (List<int>)arg[5];

        int start = workLoad * th;
        int end = (th == nrThreads - 1) ? nrItems : start + workLoad;


        for (int i = start; i < end; i++)
        {
            data[keys[i]].Sort();
        }

        print("thread=" + th + "/" + nrThreads + " with workLoad=" + workLoad + "/" + nrItems + " and data.Count=" + data.Count + " indices=[" + start + ":" + end + "]");

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
