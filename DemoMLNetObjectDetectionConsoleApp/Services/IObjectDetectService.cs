using System;
using System.Collections.Generic;
using System.Text;

namespace DemoMLNetObjectDetectionConsoleApp.Services
{
    public interface IObjectDetectService
    {
        List<string> GenerateAnalysis(string guidName);

        void CopyImageFromLocalToWebDirectory(string guidName);
    }
}
