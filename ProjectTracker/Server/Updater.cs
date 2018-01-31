
using System.Collections.Generic;
using System.ServiceModel;
using MyLib;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class Updater:IRemoteUpdate
    {

        public Updater()
        {

        }
        public List<Project> updatedProjectList()
        {
            throw new System.NotImplementedException();
        }
    }
}
