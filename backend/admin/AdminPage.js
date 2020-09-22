import AdminQuery from "./AdminQuery";
import '@babel/polyfill'
import { createDecipher } from "crypto";
import {compareSync, genSaltSync, hashSync} from 'bcryptjs';
import jwt from 'jsonwebtoken';
import {jwt_key} from "./JwtConfig"

class AdminPage {

    constructor() {
        this.query = new AdminQuery();
        this.key = jwt_key;
    }

    async checkLogin(email, password){
        try{
            let user_data = await this.query.get_user(email, 1);

            console.log(user_data);

            if(compareSync(password, user_data.password)){
                delete(user_data.password);

                let token = {data: {token: jwt.sign({ data: user_data }, this.key)}, code: 20000}

                return token;
            } else {
                throw Error("Password is invalid!");
            }

        } catch(e){
            //throw Error("User does not exists!");
            throw e;
        }
    }

    listUsers(){
        try{

            let users_info = this.query.list_users();

            return users_info;

        } catch(e){
            throw Error("Error returning user list!");
        }
    }

    listStructures(){
        try{

            let structures_list = this.query.list_structures();

            return structures_list;

        } catch(e){
            throw Error("Error returning structure list!");
        }
    }

    async saveUser(userObj){
        try{

            let salt = genSaltSync(10);
            let hash = hashSync(userObj.password, salt);
            
            userObj.password_hash = hash;
            
            let user_saved = await this.query.save_user(userObj);

            if(user_saved.affectedRows == 1){
                delete userObj.password;
                return userObj;
            }

        } catch(e){
            throw e;
        }
    }

    async editUser(id, userObj){
        try{

            if(userObj.password && userObj.password != ""){
                let salt = genSaltSync(10);
                let hash = hashSync(userObj.password, salt);
                
                userObj.password_hash = hash;
            }
            
            let user_saved = await this.query.update_user(id, userObj);

            if(user_saved.affectedRows == 1){
                delete userObj.password;
                return userObj;
            }

        } catch(e){
            throw e;
        }
    }

    userInfoToken(token){
        try{
            let decoded = jwt.verify(token, this.key);

            return decoded;
        } catch(e){
            throw e;
        }
    }

    async userInfoGet(id){
        try{
            let user_data = await this.query.get_user_id(id);

            return user_data;
        } catch(e){
            throw e;
        }
    }

    async structureInfoGet(id){
        try{
            let structure_data = await this.query.get_structure_id(id);

            return structure_data;
        } catch(e){
            throw e;
        }
    }

    async structureDeleteGet(id){
        try{
            let object_saved = await this.query.delete_structure_id(id);

            if(object_saved.affectedRows == 1){
                return {};
            } else {
                throw new Error("Issue deleting structure!");
            }
        } catch(e){
            throw e;
        }
    }

    async saveStructure(id, obj){
        try{


            let object_saved = await this.query.save_object(id, obj, 'structure');

            if(object_saved.affectedRows == 1){
                return obj;
            } else {
                throw new Error("Issue saving structure!");
            }

        } catch(e){
            throw e;
        }
    }

    async saveImport(id, obj, user){
        try{

            if(user){
                obj.admin_user_id = user.id;
            }

            let object_saved = await this.query.save_object(id, obj, 'admin_imports');

            if(object_saved.affectedRows == 1){
                obj.id = object_saved.insertId;
                return obj;
            } else {
                throw new Error("Issue saving import!");
            }

        } catch(e){
            throw e;
        }
    }

    async saveImportFile(id, obj){
        try{

            let object_saved = await this.query.save_object(id, obj, 'admin_imports');

            if(object_saved.affectedRows == 1){
                return obj;
            } else {
                throw new Error("Issue saving import!");
            }

        } catch(e){
            throw e;
        }
    }

    listImports(limit, offset){
        try{

            let list = this.query.list_imports(limit, offset);

            return list;

        } catch(e){
            throw Error("Error returning imports list!");
        }
    }

    async countImports(){
        try{

            let list = await this.query.count_imports();

            return list['total'];

        } catch(e){
            throw Error("Error counting imports list!");
        }
    }

    listImportLogs(id){
        try{

            let list = this.query.list_import_logs(id);

            return list;

        } catch(e){
            throw Error("Error returning imports list!");
        }
    }

    async importsInfoGet(id){
        try{
            let data = await this.query.get_import_id(id);

            return data;
        } catch(e){
            throw e;
        }
    }

    async saveExport(id, obj, user){
        try{

            if(user){
                obj.admin_user_id = user.id;
            }

            let object_saved = await this.query.save_object(id, obj, 'admin_exports');

            if(object_saved.affectedRows == 1){
                obj.id = object_saved.insertId;
                return obj;
            } else {
                throw new Error("Issue saving export!");
            }

        } catch(e){
            throw e;
        }
    }

    listExports(limit, offset){
        try{

            let list = this.query.list_exports(limit, offset);

            return list;

        } catch(e){
            throw Error("Error returning exports list!");
        }
    }

    async countExports(){
        try{

            let list = await this.query.count_exports();

            return list['total'];

        } catch(e){
            throw Error("Error returning exports list!");
        }
    }

    listCrons(type){
        try{

            let list = this.query.list_crons(type);

            return list;

        } catch(e){
            throw Error("Error returning cron list!");
        }
    }

    async gamemodesInfoGet(){
        try{
            let data = await this.query.get_gamemodes();

            return data;
        } catch(e){
            throw e;
        }
    }

    async saveGamemodes(obj){
        try{

            let object_saved = await this.query.save_object(1, obj, 'admin_gamemodes');

            if(object_saved.affectedRows == 1){
                return obj;
            } else {
                throw new Error("Issue saving gamemodes!");
            }

        } catch(e){
            throw e;
        }
    }
}

module.exports = AdminPage;