
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using MyLib;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    class Updater:IRemoteUpdate
    {       
        private ServerClass mServer;
        public Updater()
        {
            mServer = new ServerClass();
        }

        public Updater(ServerClass server)
        {
            mServer = server;
        }

        public List<Project> updatedProjectList()
        {
            return mServer.GetProjectList();
        }

        public void SetServer(ServerClass server)
        {
            mServer = server;
        }

    }
}
