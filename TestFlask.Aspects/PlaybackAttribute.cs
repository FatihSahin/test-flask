using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlask.Aspects.Identifiers;

namespace TestFlask.Aspects
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class PlaybackAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        /// <summary>
        /// Test Flask Aspect 
        /// </summary>
        /// <param name="requestIdentiferType">A  type that implements IRequestIdentifer</param>
        /// <param name="responseIdentifierType">A type that implements IResponseIdentifier</param>
        public PlaybackAttribute(Type requestIdentiferType = null, Type responseIdentifierType = null)
        {
            if (requestIdentiferType != null)
            {
                if (!typeof(IRequestIdentifier).IsAssignableFrom(requestIdentiferType))
                {
                    throw new ArgumentException("Request identifier type must implement IRequestIdentifer<T>");
                }
            }

            if (responseIdentifierType != null)
            {
                if (!typeof(IResponseIdentifier<>).IsAssignableFrom(responseIdentifierType))
                {
                    throw new ArgumentException("Response identifier type must implement IResponseIdentifier<T>");
                }
            }
        }
    }
}
