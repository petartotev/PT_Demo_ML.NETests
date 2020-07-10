namespace DemoMLNetObjectDetectionConsoleApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IObjectDetectService
    {
        List<string> GenerateAnalysis(string guidName);

        void CopyImageFromLocalToWebDirectory(string guidName);
    }
}
