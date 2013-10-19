using System;
using System.ServiceModel;

namespace Service
{
    public static class Proxy<TChannel>
    {
        /// <summary>
        /// Replaces the common but incorrect "using (ServiceClient sc = new ServiceClient()) {}" usage pattern for WCF
        /// web service proxies.
        /// For an explanation of why this is necessary read http://msdn.microsoft.com/en-us/library/aa355056.aspx
        /// This is essentially sugar on top of Microsoft's recommentation courtesy of http://stackoverflow.com/a/16445534/1169863
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value of the invocated code block <paramref name="codeBlock"/></typeparam>
        /// <param name="constructor">Specifies a function that can construct a service channel.</param>
        /// <param name="codeBlock">
        /// The code to execute in the context of the service proxy. 
        /// The first argument to this function is the service channel that is returned by the <paramref name="constructor"/> code block.
        /// </param>
        /// <returns>The value returned by the invocated code block <paramref name="codeBlock"/>.</returns>
        /// <remarks>
        /// Use this overload if the default channel factory constructor is not appropriate and the executing code block
        /// should return a value.
        /// </remarks>
        public static TReturn Use<TReturn>(Func<TChannel> constructor, Func<TChannel, TReturn> codeBlock)
        {
            var proxy = (ICommunicationObject)constructor();
            var success = false;
            try
            {
                var result = codeBlock((TChannel)proxy);
                proxy.Close();
                success = true;
                return result;
            }
            finally
            {
                if (!success)
                {
                    proxy.Abort();
                }
            }
        }

        /// <summary>
        /// Replaces the common but incorrect "using (ServiceClient sc = new ServiceClient()) {}" usage pattern for WCF
        /// web service proxies.
        /// For an explanation of why this is necessary read http://msdn.microsoft.com/en-us/library/aa355056.aspx
        /// This is essentially sugar on top of Microsoft's recommentation courtesy of http://stackoverflow.com/a/16445534/1169863
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value of the invocated code block <paramref name="codeBlock"/></typeparam>
        /// <param name="codeBlock">
        /// The code to execute in the context of the service proxy. 
        /// The first argument to this function is the service channel that is opened by the default channel factory constructor.
        /// </param>
        /// <returns>The value returned by the invocated code block <paramref name="codeBlock"/>.</returns>
        /// <remarks>
        /// Use this overload if the executing code block should return a value.
        /// </remarks>
        public static TReturn Use<TReturn>(Func<TChannel, TReturn> codeBlock)
        {
            ChannelFactory<TChannel> channelFactory = new ChannelFactory<TChannel>("*");
            return Use<TReturn>(() => { return (TChannel)channelFactory.CreateChannel(); }, codeBlock);
        }

        /// <summary>
        /// Replaces the common but incorrect "using (ServiceClient sc = new ServiceClient()) {}" usage pattern for WCF
        /// web service proxies.
        /// For an explanation of why this is necessary read http://msdn.microsoft.com/en-us/library/aa355056.aspx
        /// This is essentially sugar on top of Microsoft's recommentation courtesy of http://stackoverflow.com/a/16445534/1169863
        /// </summary>
        /// <param name="constructor">Specifies a function that can construct a service channel.</param>
        /// <param name="codeBlock">
        /// The code to execute in the context of the service proxy.
        /// The first argument to this function is the service channel that is opened by the <paramref name="constructor"/> function block.
        /// </param>
        /// <remarks>
        /// Use this overload if the default channel factory constructor is not appropriate and the executing code block
        /// should not return a value.
        /// </remarks>
        public static void Use(Func<TChannel> constructor, Action<TChannel> codeBlock)
        {
            Use(constructor, proxy =>
            {
                codeBlock(proxy);
                return true;
            });
        }

        /// <summary>
        /// Replaces the common but incorrect "using (ServiceClient sc = new ServiceClient()) {}" usage pattern for WCF 
        /// web service proxies.
        /// For an explanation of why this is necessary read http://msdn.microsoft.com/en-us/library/aa355056.aspx
        /// This is essentially sugar on top of Microsoft's recommentation courtesy of http://stackoverflow.com/a/16445534/1169863
        /// </summary>
        /// <param name="codeBlock">
        /// The code to execute in the context of the service channel.
        /// The first argument to this function is the service channel that is opened by the default channel factory constructor.
        /// </param>
        /// <remarks>
        /// Use this overload if the executing code block should not return a value.
        /// </remarks>
        public static void Use(Action<TChannel> codeBlock)
        {
            ChannelFactory<TChannel> channelFactory = new ChannelFactory<TChannel>("*");
            Use(() => { return (TChannel)channelFactory.CreateChannel(); }, codeBlock);
        }
    }
}
