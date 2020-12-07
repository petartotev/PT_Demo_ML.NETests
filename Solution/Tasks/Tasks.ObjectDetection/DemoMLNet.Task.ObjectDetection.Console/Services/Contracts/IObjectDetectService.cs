namespace DemoMLNetObjectDetectionConsoleApp.Services
{
    using System.Collections.Generic;

    public interface IObjectDetectService
    {
        List<string> GenerateAnalysis(string guidName);

        void CopyImageFromLocalToWebDirectory(string guidName);
    }
}
