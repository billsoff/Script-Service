(function ($) {
    function main() {
        render(this.sh, this.canvas);
    }

    window['1E37E794-301E-4C41-8CC1-B418ED453617'] = main;

    function render(sh, canvas) {
        sh.highlight(null, document.getElementById(canvas));
    }
})(jQuery);
