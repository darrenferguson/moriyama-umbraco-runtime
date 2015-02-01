

Modernizr.load([{
    test: Modernizr.fontface,
    complete: function () {
        TypekitConfig = {
            kitId: "vpg1cgh",
            scriptTimeout: 3e3
        },
        function () {
            var e = setTimeout(function () { }, TypekitConfig.scriptTimeout),
                t = document.createElement("script");
            t.src = "//use.typekit.com/" + TypekitConfig.kitId + ".js", t.onload = t.onreadystatechange = function () {
                var t = this.readyState;
                if (t && t != "complete" && t != "loaded") return;
                clearTimeout(e);
                try {
                    Typekit.load(TypekitConfig)
                } catch (n) { }
            };
            var n = document.getElementsByTagName("script")[0];
            n.parentNode.insertBefore(t, n)
        }()
    }
}, {
    test: Modernizr.mq("only all"),
    nope: "/scripts/vendor/respond.min.js"
}, {
    load: "//ajax.googleapis.com/ajax/libs/jquery/1.8.0/jquery.min.js",
    complete: function () {
        window.jQuery || Modernizr.load("/scripts/vendor/jquery-1.8.0.min.js");
        SyntaxHighlighter.defaults['toolbar'] = false;
        SyntaxHighlighter.all();
        $('#moriyama1').hide();
    }
}, {
    load: "ielt9!/scripts/vendor/selectivizr-1.0.3.min.js",
    complete: function () {
        $(function () {
            $('[role="banner"]').append('<a href="#menu" id="menu-link"><i class="icon-reorder"></i></a>');
            var e = $("#menu"),
                t = $("#menu-link");
            t.click(function () {
                return t.toggleClass("active"), e.toggleClass("active"), !1
            })
        })
    }
}]);


   