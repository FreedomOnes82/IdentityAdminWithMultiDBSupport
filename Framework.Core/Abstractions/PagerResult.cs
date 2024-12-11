namespace Framework.Core.Abstractions
{
    public class PagerResult<M>
    {
        public int TotalCount { get; set; }
        public IEnumerable<M> Results { get; set; }
    }
}
