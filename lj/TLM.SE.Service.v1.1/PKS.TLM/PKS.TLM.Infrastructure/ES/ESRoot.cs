namespace PKS.WebAPI.ES.Model.EsRawResult
{
    public class ESRoot
    {
        public double? took { get; set; }
        public bool timed_out { get; set; }
        public object _shards { get; set; }
        public ESHit hits { get; set; }
    }
}
