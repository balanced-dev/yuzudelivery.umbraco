module.exports = {
    source: {
        name: "trello",
        settings: {
            board: "{TRELLO BOARD NAME}",
            key: "••••••••••••••••••••••••••••••••",
            secret: "••••••••••••••••••••••••••••••••••••••••••••••••••••••••••••••••"
        }
    },
    modules: [
        'yuzu', 'schema', 'scss.bem', 'hbs.settings', 'hbs.prettier',
    ]
}