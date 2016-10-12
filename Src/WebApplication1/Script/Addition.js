(function ($) {
    function main() {
        return add;
    }

    window['7D742C4A-49C9-4067-85F8-8EF1CA389CC6'] = main;

    function add(x, y) {
        if (arguments.length < 2) {
            return x;
        }

        return (x + y);
    }
})(jQuery);
