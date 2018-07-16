function checkStrength(password) {
    //initial strength
    var strength = 0;
    if (password.length == 0)
        strength = 0;
    //if the password length is less than 6, return message.
    if (password.length > 0 && password.length < 6)
        strength = 1;

    //if length is 6 characters or more, increase strength value
    if (password.length > 5) strength = 2;

    //if password contains both lower and uppercase characters, increase strength value
    if (password.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)) strength += 1

    //if it has numbers and characters, increase strength value
    if (password.match(/([a-zA-Z])/) && password.match(/([0-9])/)) strength += 1

    //if it has one special character, increase strength value
    if (password.match(/([!,%,&,@@,#,$,£,^,*,?,_,~,+,-,/])/)) strength += 1

    //if it has two special characters, increase strength value
    if (password.match(/(.*[!,%,&,@@,#,$,£,^,*,?,_,~,+,-,/].*[!,%,&,@@,#,$,£,^,*,?,_,~,+,-,/])/)) strength += 1

    return strength;
}
