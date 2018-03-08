namespace OpenTl.ClientApi.MtProto.FunctionalTests.Framework
{
    using System;
    using System.IO;
    using System.Reflection;

    using DotNetty.Common.Internal.Logging;

    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.Common.Testing.Logger;

    using Xunit.Abstractions;

    internal sealed class TestOutputAppender : AppenderSkeleton
    {
        private readonly ITestOutputHelper _xunitTestOutputHelper;

        public TestOutputAppender(ITestOutputHelper xunitTestOutputHelper)
        {
            _xunitTestOutputHelper = xunitTestOutputHelper;
            Name = "TestOutputAppender";
            Layout = new PatternLayout("%-5p %d %5rms %-22.22c{1} %-18.18M - %m%n");
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            _xunitTestOutputHelper.WriteLine(RenderLoggingEvent(loggingEvent));
        }
    }

    internal sealed class LogOutputTester : IDisposable
    {
        private readonly TestOutputAppender _appender;

        private readonly IAppenderAttachable _attachable;

        public LogOutputTester(ITestOutputHelper output)
        {
            var repo = LogManager.GetRepository(typeof(INettyBootstrapper).GetTypeInfo().Assembly);
            XmlConfigurator.Configure(repo, new FileInfo("log4net.config"));

            InternalLoggerFactory.DefaultFactory.AddProvider(new Log4NetProvider(repo));
                
            var root = ((Hierarchy)repo).Root;
            _attachable = root;

            _appender = new TestOutputAppender(output);
            _attachable?.AddAppender(_appender);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _attachable.RemoveAppender(_appender);
        }
    }
}