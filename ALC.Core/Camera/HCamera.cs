﻿using System;
using System.Threading.Tasks;
using HalconDotNet;

namespace ALC.Core.Camera
{
    public class HCamera
    {
        #region private members

        /// <summary>
        /// For example, "DahuaTechnology:5C02A78YAKB6AE6"
        /// </summary>
        public string Name { get; set; }
        private bool _isGrabbing = true;
        private HTuple _cameraHandle;

        #endregion

        public event Action<HObject> SingleImageAcquired; 


        #region ctor

        public HCamera(string name)
        {
            Name = name;
        }

        #endregion
        
        

        public void OpenAndGrabContinuously()
        {
            // Local control variables 

            // Initialize local and output iconic variables 
            //Image Acquisition 01: Code generated by Image Acquisition 01
            HOperatorSet.OpenFramegrabber("HMV3rdParty", 0, 0, 0, 0, 0, 0, "progressive", 
                -1, "default", -1, "false", "default", Name, 
                0, -1, out _cameraHandle);
            SetHardwareTriggerMode(_cameraHandle);
            
            
            Task.Run(() =>
            {
                while (_isGrabbing)
                {
                    HOperatorSet.GrabImage(out var hObj, _cameraHandle);
                    //Image Acquisition 01: Do something
                    OnSingleImageAcquired(hObj);
                }

                HOperatorSet.CloseFramegrabber(_cameraHandle);

            });

        }


        public void SetHardwareTriggerMode(HTuple cameraHandle)
        {
            HOperatorSet.SetFramegrabberParam(cameraHandle,  "TriggerSelector", "FrameStart");
            HOperatorSet.SetFramegrabberParam(cameraHandle,  "TriggerMode", "On");
            HOperatorSet.SetFramegrabberParam(cameraHandle,   "TriggerSource", "Line1");
            HOperatorSet.SetFramegrabberParam(cameraHandle,   "grab_timeout", -1);
        }
        
        public void StopAndClose()
        {
            _isGrabbing = false;
        }

        protected virtual void OnSingleImageAcquired(HObject obj)
        {
            SingleImageAcquired?.Invoke(obj);
        }
    }
}