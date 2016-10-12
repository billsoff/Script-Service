(function ($) {
    function main() {
        return render(this.getSH(), this.canvas);

        render(this.getSH(), this.canvas);
    }

    window['B4CDDB31-1C56-46BB-80F2-225C8B1C475B'] = main;

    function render(sh, canvas) {
        sh.highlight(null, document.getElementById(canvas));
    }
})(jQuery);
