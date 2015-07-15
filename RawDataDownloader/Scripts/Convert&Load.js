db.Events.aggregate(
[
    {
        $project:
        {
            _id: 1,
            event: 1,
            properties: 1,
            time: {
                $add: [new Date(0), {
                    $multiply: ["$properties.time", 1000]
                }]
            }
        }
    },
    {
        $out: "EventsConverted"
    }
]
)