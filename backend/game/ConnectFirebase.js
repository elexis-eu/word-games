var admin = require('firebase-admin');


class ConnectFirebase {
    constructor() {
        this.serviceAccount = require('../igra-besed-testing-firebase-adminsdk-ez1lx-87fc0912ee.json');

        admin.initializeApp({
            credential: admin.credential.cert(this.serviceAccount),
            databaseURL: 'https://igra-besed-testing.firebaseio.com'
        });
    }


    register(idToken, params) {
        admin.auth().verifyIdToken(idToken)
            .then(function(decodedToken) {
                var uid = decodedToken.uid;
                // ...
            }).catch(function(error) {
            // Handle error
        });
    }
}





module.exports = ConnectFirebase;