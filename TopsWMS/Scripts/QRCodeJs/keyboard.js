$(function () {
    $("input.keyboardContent[type='text'],input.keyboardContent[type='password']").click(function () {
        $("input[title=write]").removeAttr("title");
        $(this).attr("title", "write");
    })

    $('.keyboard li').click(function () {
        $("input[title=write]").focus();
        var $write = $('input[title=write]'),
		capslock = false;
        var $this = $(this),
			character = $this.html();

        // Caps lock
        if ($this.hasClass('capslock')) {
            $('.letter').toggleClass('uppercase');
            capslock = false;
            return false;
        }

        // Delete
        if ($this.hasClass('delete')) {
            var html = $write.val();
            $write.val(html.substr(0, html.length - 1));
            $write.change();
            return false;
        }


        // Uppercase letter
        if ($this.hasClass('uppercase')) character = character.toLowerCase();

        // Add the character
        $write.val($write.val() + character);
        $write.change();
    });
});
