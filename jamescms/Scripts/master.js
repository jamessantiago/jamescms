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

function textWallInfiniScroll() {
    
    $(window).scroll(function () {
        if (window.EnableScroll)
        {
            var currentPosition = window.pageYOffset;
            var bottom = $(window).height() - 100;
            if (currentPosition > bottom) {
                var lastPage = $(".page:last").attr("page-number");
                if (lastPage.length > 0)
                {
                    window.EnableScroll = false;
                    $("#textWall").append($("<div>").load("text/p/" + lastPage, function () {
                        if (!window.EndOfPage) {
                            window.EnableScroll = true;
                            textWallInfiniScroll();
                        }
                    }));
                }
            }
        }
    });
}

function previewMarkdown(text, preview, url) {
    $(text).keydown(function () {        
        if ($(preview).hasClass("loading") == false)
        {
            $(preview).addClass("loading");
            $.ajax({
                type: "POST",
                url: url,
                data: { "data": $(text).val() },
                dataType: "Html",
                traditional: true,
                success: function (data) {
                    $(preview).removeClass("loading");
                    $(preview).html(data);
                }
            });
        }
    });
}


function readyControl() {
    prettyPrint();
    jLoad();
    readyLoad();
}
