﻿using System.Net.Sockets;

namespace IOTController
{
    public interface IController
    {
        void ChangeWindowStatus(int GreenHouseId, bool status);
    }
}