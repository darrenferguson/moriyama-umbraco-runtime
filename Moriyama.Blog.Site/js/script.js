

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

window._gaq = [['_setAccount', 'UA-26468870-1'], ['_trackPageview'], ['_trackPageLoadTime']];
Modernizr.load({
    load: ('https:' == location.protocol ? '//ssl' : '//www') + '.google-analytics.com/ga.js'
});

!function () {
    var analytics = window.analytics = window.analytics || []; if (!analytics.initialize) if (analytics.invoked) window.console && console.error && console.error("Segment snippet included twice."); else {
        analytics.invoked = !0; analytics.methods = ["trackSubmit", "trackClick", "trackLink", "trackForm", "pageview", "identify", "group", "track", "ready", "alias", "page", "once", "off", "on"]; analytics.factory = function (t) { return function () { var e = Array.prototype.slice.call(arguments); e.unshift(t); analytics.push(e); return analytics } }; for (var t = 0; t < analytics.methods.length; t++) { var e = analytics.methods[t]; analytics[e] = analytics.factory(e) } analytics.load = function (t) { var e = document.createElement("script"); e.type = "text/javascript"; e.async = !0; e.src = ("https:" === document.location.protocol ? "https://" : "http://") + "cdn.segment.com/analytics.js/v1/" + t + "/analytics.min.js"; var n = document.getElementsByTagName("script")[0]; n.parentNode.insertBefore(e, n) }; analytics.SNIPPET_VERSION = "3.0.1";
        analytics.load("QgDmGvCjQv4rLIIfAQ0WTfbe8CAUfUtO");
        analytics.page()
    }
}();