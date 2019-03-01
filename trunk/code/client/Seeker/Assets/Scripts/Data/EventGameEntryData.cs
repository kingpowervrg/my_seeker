using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{

    public class EventGameEntryData
    {
        public long M_event_id { get; set; }
        public List<long> M_normal_drops { get; set; }
        public List<long> M_full_drops { get; set; }

    }

    public class EventGamePlayData
    {
        public long EventID {get; set;}
        public SCEventPhaseFeedbackResponse Msg { get; set; }

    }


}

