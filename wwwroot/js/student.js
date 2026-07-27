document.addEventListener("DOMContentLoaded", function () {

    const sendBtn = document.getElementById("sendOtpBtn");
    const verifyBtn = document.getElementById("verifyOtpBtn");
    const saveBtn = document.getElementById("saveBtn");

    // ==========================
    // Auto Calculate Age
    // ==========================
    const dob = document.getElementById("dob");
    const age = document.getElementById("age");

    if (dob && age) {

        dob.addEventListener("change", function () {

            if (this.value === "") {
                age.value = "";
                return;
            }

            let birthDate = new Date(this.value);
            let today = new Date();

            let years = today.getFullYear() - birthDate.getFullYear();

            let month = today.getMonth() - birthDate.getMonth();

            if (month < 0 || (month === 0 && today.getDate() < birthDate.getDate())) {
                years--;
            }

            age.value = years;
        });
    }

    // ==========================
    // Send OTP
    // ==========================
    if (sendBtn) {

        sendBtn.addEventListener("click", function () {

            let email = document.getElementById("email").value;

            if (email === "") {
                alert("Please enter email.");
                return;
            }

            fetch('/Students/SendOtp', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body: 'email=' + encodeURIComponent(email)
            })
                .then(r => r.json())
                .then(data => {
                    alert(data.message);
                });

        });

    }

    // ==========================
    // Verify OTP
    // ==========================
    if (verifyBtn) {

        verifyBtn.addEventListener("click", function () {

            let email = document.getElementById("email").value;
            let otp = document.getElementById("otp").value;

            fetch('/Students/VerifyOtp', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body:
                    'email=' + encodeURIComponent(email) +
                    '&otp=' + encodeURIComponent(otp)
            })
                .then(r => r.json())
                .then(data => {

                    alert(data.message);

                    if (data.success) {

                        saveBtn.disabled = false;

                        let otpVerified = document.getElementById("OtpVerified");

                        if (otpVerified) {
                            otpVerified.value = "true";
                        }

                    }

                });

        });

    }

});