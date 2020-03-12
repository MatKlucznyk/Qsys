using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysCameraSimpl
    {
        private QsysCamera camera;

        public void Initialize(string name)
        {
            camera = new QsysCamera(name);
        }

        public void TiltUp()
        {
            camera.StartPTZ(QsysCamera.PtzTypes.Up);
        }

        public void StopTiltUp()
        {
            camera.StopPTZ(QsysCamera.PtzTypes.Up);
        }

        public void TiltDown()
        {
            camera.StartPTZ(QsysCamera.PtzTypes.Down);
        }

        public void StopTiltDown()
        {
            camera.StopPTZ(QsysCamera.PtzTypes.Down);
        }

        public void PanLeft()
        {
            camera.StartPTZ(QsysCamera.PtzTypes.Left);
        }

        public void StopPanLeft()
        {
            camera.StopPTZ(QsysCamera.PtzTypes.Left);
        }

        public void PanRight()
        {
            camera.StartPTZ(QsysCamera.PtzTypes.Right);
        }

        public void StopPanRight()
        {
            camera.StopPTZ(QsysCamera.PtzTypes.Right);
        }

        public void RecallHome()
        {
            camera.RecallHome();
        }
    }
}