using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace TMCurve.MyClass
{
    //class Mythread
    //{
    //    public Thread thrd;
    //    //创建一个可授权2个许可证的信号量，且初始值为2
    //   // public static Semaphore sem = new Semaphore(3, 3);
    //    public Mythread(Object ob)
    //    {
    //        importDTS t = new importDTS();
    //        thrd = new Thread(new ParameterizedThreadStart(t.insertData));
    //        thrd.Start(ob);
    //        //WaitCallback callBack;
    //        //callBack = new WaitCallback(t.insertData);
    //        //ThreadPool.QueueUserWorkItem(callBack,ob);

    //    }
    //}
    class MyThread
    {
        Thread t = null;
        ManualResetEvent manualEvent = new ManualResetEvent(true);//为true,一开始就可以执行
        private void Run()
        {
            while (true)
            {
                this.manualEvent.WaitOne();
                Console.WriteLine("这里是  {0}", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(5000);
            }
        }

        public void Start()
        {
            this.manualEvent.Set();
        }

        public void Stop()
        {
            this.manualEvent.Reset();
        }
        public MyThread()
        {
            t = new Thread(this.Run);
            t.Start();
        }
    }
}
