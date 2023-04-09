using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperations
{
    public class ParameterCollection
    {
        private List<Parameter> _parameterCollection = new List<Parameter>();

        /// <summary>
        /// Adds a DBParameter to the ParameterCollection
        /// </summary>
        /// <param name="parameter">Parameter to be added</param>
        public void Add(Parameter parameter)
        {
            _parameterCollection.Add(parameter);
        }

        /// <summary>
        /// Removes parameter from the Parameter Collection
        /// </summary>
        /// <param name="parameter">Parameter to be removed</param>
        public void Remove(Parameter parameter)
        {
            _parameterCollection.Remove(parameter);
        }

        /// <summary>
        /// Removes all the parameters from the Parameter Collection
        /// </summary>
        public void RemoveAll()
        {
            _parameterCollection.RemoveRange(0, _parameterCollection.Count - 1);
        }

        /// <summary>
        /// Removes parameter from the specified index.
        /// </summary>
        /// <param name="index">Index from which parameter is supposed to be removed</param>
        public void RemoveAt(int index)
        {
            _parameterCollection.RemoveAt(index);
        }

        /// <summary>
        /// Gets list of parameters
        /// </summary>
        internal List<Parameter> Parameters
        {
            get
            {
                return _parameterCollection;
            }
        }
    }
}
