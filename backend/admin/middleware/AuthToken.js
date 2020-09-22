import jwt from 'jsonwebtoken'
import {jwt_key} from "../JwtConfig"

let checkAdminToken = (req, res, next) => {

  if(!req.headers['authorization']){
    return res.json({
      success: false,
      message: 'Auth token is not supplied'
    });    
  }

  let token =  req.headers['authorization']; // Express headers are auto converted to lowercase
  if (token.startsWith('Bearer ')) {
    // Remove Bearer from string
    token = token.slice(7, token.length);
  }

  if (token) {
    jwt.verify(token, jwt_key, (err, decoded) => {
      if (err) {
        return res.json({
          success: false,
          message: 'Token is not valid'
        });
      } else {
        req.user = decoded.data;
        next();
      }
    });
  } else {
    return res.json({
      success: false,
      message: 'Auth token is not supplied'
    });
  }
};

module.exports = {
  checkAdminToken: checkAdminToken
}