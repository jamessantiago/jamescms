PR.registerLangHandler(
    PR.createSimpleLexer(
        [ // shortcutStylePatterns
        ],
        [ // fallthroughStylePatterns
            //Error lines
            [PR.PR_KEYWORD, /.*(?:ERROR| AT |WARN|FATAL|ISSUE|FAIL).*/i, null],
            //Normal lines
            [PR.PR_STRING, /.*(?:TRACE|DEBUG|INFO).*/i, null]            
        ]),
    ['logfile', 'log']);