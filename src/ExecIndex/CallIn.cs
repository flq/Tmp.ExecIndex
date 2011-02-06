using System.Collections.Generic;

namespace ExecIndex
{
    public interface CallIn
    {
        void Install(string name, IList<string> dependencies);
        void Uninstall(int sequence, string name, IList<string> dependencies);
    }
}