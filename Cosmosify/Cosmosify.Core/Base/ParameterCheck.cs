using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cosmosify.Core.Base
{
    public static class ParameterCheck
    {
        public static void CheckValidation<T>(this (T[] buffer, int offset, int count) copyTo)
        {
            var (buffer, offset, count) = copyTo;

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "Buffer should not be null.");
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset should between 0 and buffer's length.");
            }
            if (count < 0 || count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Count should between 0 and buffer's length minus offset.");
            }
        }
    }
}
