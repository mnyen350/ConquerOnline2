using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConquerServer.Database.Models
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public class SpawnCircleModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("mapid")]
        public int MapId { get; set; }

        [JsonPropertyName("bound_x")]
        public int X { get; set; }

        [JsonPropertyName("bound_y")]
        public int Y { get; set; }

        [JsonPropertyName("bound_cx")]
        public int XLimit { get; set; }

        [JsonPropertyName("bound_cy")]
        public int YLimit { get; set; }

        [JsonPropertyName("maxnpc")]
        public int MaxMonsterCount { get; set; }

        [JsonPropertyName("test_secs")]
        public int SpawnCooldown { get; set; }

        [JsonPropertyName("max_per_gen")]
        public int MaxPerGen { get; set; }//??

        [JsonPropertyName("npctype")]
        public int MonsterTypeORId { get; set; } //are you an id? a type?

        [JsonPropertyName("timer_begin")]
        public int TimerBegin { get; set; }

        [JsonPropertyName("timer_end")]
        public int TimerEnd { get; set; }

        [JsonPropertyName("born_x")]
        public int BornX { get; set; }

        [JsonPropertyName("born_y")]
        public int BornY { get; set; }

        [JsonPropertyName("mask")]
        public int Mask { get; set; }//are you a model?(part of lookface?) ??

        [JsonPropertyName("path")]
        public int Path { get; set; }

        [JsonPropertyName("return_mode")]
        public int ReturnMode { get; set; }

        [JsonPropertyName("cluster_type")]
        public int ClusterType { get; set; } 

    }
}
