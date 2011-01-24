using System.Collections.Generic;

namespace ExecIndex
{
    public interface CallIn
    {
        void Install(string name, IList<string> dependencies);
    }
}