local skateboardTrigger = {}

skateboardTrigger.name = "Skateline/SkateboardTrigger"
skateboardTrigger.placements = {
    name = "skateboard_trigger",
    data = {
        mode = "enable"
    }
}

skateboardTrigger.fieldInformation = {
    mode = {
        options = {
            "enable",
            "disable",
            "toggle"
        },
        editable = false
    }
}

return skateboardTrigger
