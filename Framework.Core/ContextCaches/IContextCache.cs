using System.Collections.Generic;

namespace Framework.Core.ContextCaches
{
    public interface IContextCache
    {
        M Get<M>(string key);

        object Get(string key);

        void Set<M>(string key, M value);

        void Remove(string key);

        List<string> Keys { get;  }
    }
}
