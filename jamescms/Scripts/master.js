$(document).ready(function () {
    prettyPrint();
    jLoad();
    readyLoad();
});

function jLoad() {
    $(".jLoad").click(function () {
        $($(this).attr("target")).html("<img src='" + $(this).attr("href-loading") + "' alt='loading' /> Loading");
        $($(this).attr("target")).load($(this).attr("href-to-load"))
    });
}

function readyLoad()
{
    $(".readyLoad").each(function () {
        var currHtml = $(this).html()
        if (currHtml.length == 0 || currHtml == "")
        {
            $(this).html("<img src='" + $(this).attr("href-loading") + "' alt='loading' /> Loading");
            $(this).load($(this).attr("href-to-load"), function ()
            {
                prettyPrint();
                jLoad();
                readyLoad();
            });
        }        
    });
}