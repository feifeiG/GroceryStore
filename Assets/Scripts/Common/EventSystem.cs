using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystem{

    //设计思路模拟CS的流程，便于以后多人联机。
    //客户端自身的事件用1-10000编号，服务端的事件用10001-20000，客户端服务端交互的事件用20001-40000
    public enum EventType{
        //Client
        ClientInit = 1,

        //Server
        ServerInit = 10001,

        //Client Server Connect
        CS_Login = 20001,

        SC_Logic = 30001,
    }

    public class Event<T>
    {
        public int EventId = 0;
        public int ProducerId = 0;
        public List<T> Param;
    }

    public class EventDispatcher
    {
        public static EventDispatcher instance = null;

        public static EventDispatcher Getinstance()
        {
            if(instance == null){
                instance = new EventDispatcher();
            }
            return instance;
        }

        public void RegisterListen(int event_id)
        {

        }

        public void DispatcEvent(Event e)
        {

        }
    }
}
