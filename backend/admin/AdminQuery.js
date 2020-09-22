import mysql from "mysql";
import util from "util";

class AdminQuery {

    constructor() {

        let host = process.env.IGRA_BESED_DATABASE_HOST || 'localhost';
        let port = process.env.IGRA_BESED_DATABASE_PORT || '3306';
        let user = process.env.IGRA_BESED_DATABASE_USER || 'root';
        let passwd = process.env.IGRA_BESED_DATABASE_PASSWD || 'root';
        let schema = process.env.IGRA_BESED_DATABASE_SCHEMA || 'igra_english';

        // console.log("query PASS: ", passwd);

        this.connection = mysql.createConnection({
            host     : host,
            port     : port,
            user     : user,
            password : passwd,
            insecureAuth: true,
            database : schema,
            timezone : 'utc'
        });

        this.connection.connect((err) => {
            if(!err) {
                console.log("Database is connected ...");
            } else {
                console.log("Error when connecting on database ...");
                throw err;
            }
        });
    }

    async get_one(sql, args){
        let result = await util.promisify( this.connection.query )
        .call( this.connection, sql, args );

        if(result.length == 1){
            return result[0];
        } else {
            throw new Error("Not exactly one result for get one!");
        }
    }

    list_users(){
        
        let sql =   "SELECT id, title, email, role, active, created FROM admin_user";

        let args = [];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    save_user(user_obj){
        
        let sql =   "INSERT INTO admin_user (title, email, password, role, active, created) VALUES (?, ?, ?, ?, ?, NOW())";

        let args = [user_obj.title, user_obj.email, user_obj.password_hash, user_obj.role, 1];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );
    }

    update_user(id, user_obj){

        let sql = "";
        let args = [];
        
        if(user_obj.password_hash){
            sql =   "UPDATE admin_user SET title =?, email=?, password=?, role=?, active=? WHERE id = ?";

            args = [user_obj.title, user_obj.email, user_obj.password_hash, user_obj.role, user_obj.active, id];
        } else {
            sql =   "UPDATE admin_user SET title =?, email=?, role=?, active=? WHERE id = ?";

            args = [user_obj.title, user_obj.email, user_obj.role, user_obj.active, id];
        }

        console.log(sql);
        console.log(args);

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );
    }

    async get_user(email, active){
        
        let sql =   "SELECT * FROM admin_user WHERE email = ? AND active = ?";

        let args = [email, active];

        try{
            return await this.get_one(sql, args);
        } catch(exception){
            return Error("Error getting user!");                        
        }
    }

    async get_user_id(id){
        
        let sql =   "SELECT title, email, role, active FROM admin_user WHERE id = ?";

        let args = [id];

        try{
            return await this.get_one(sql, args);
        } catch(exception){
            return Error("Error getting user!");                        
        }
    }

    async get_structure_id(id){
        
        let sql =   "SELECT * FROM structure WHERE id = ?";

        let args = [id];

        try{
            return await this.get_one(sql, args);
        } catch(exception){
            return Error("Error getting structure!");                        
        }
    }

    async delete_structure_id(id){
        
        let sql =   "DELETE FROM structure WHERE id = ?";

        let args = [id];

        return util.promisify( this.connection.query )
        .call( this.connection, sql, args );
    }

    
    list_structures(){
        
        let sql =   "SELECT * FROM structure";

        let args = [];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    list_imports(limit, offset){
        
        let sql =   "SELECT ai.*, au.email FROM admin_imports ai INNER JOIN admin_user au ON au.id = ai.admin_user_id ORDER BY ai.created DESC LIMIT ? OFFSET ?";

        let args = [limit, offset];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    async count_imports(){
        
        let sql =   "SELECT count(ai.id) as total FROM admin_imports ai INNER JOIN admin_user au ON au.id = ai.admin_user_id";

        let args = [];

        return await this.get_one(sql, args);
    }

    list_import_logs(id){
        
        let sql =   "SELECT * FROM admin_report_log WHERE import_id = ?";

        let args = [id];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    save_object(id, data, tableName, idName){
        
        let sql = "";

        if(!idName){
            idName = 'id';
        }
        
        if(id){
            sql =  "UPDATE " + tableName + " SET ? WHERE " + idName + " = " + id;
        } else {
            sql =  "INSERT INTO " + tableName + " SET ?";
        }

        //let args = [user_obj.title, user_obj.email, user_obj.password_hash, user_obj.role, 1];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, data );
    }

    async get_import_id(id){
        
        let sql =   "SELECT * FROM admin_imports WHERE id = ?";

        let args = [id];

        try{
            return await this.get_one(sql, args);
        } catch(exception){
            return Error("Error getting structure!");                        
        }
    }

    list_exports(limit, offset){
        
        let sql =   "SELECT ae.*, au.email FROM admin_exports ae INNER JOIN admin_user au ON au.id = ae.admin_user_id ORDER BY ae.created DESC LIMIT ? OFFSET ?";

        let args = [limit, offset];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    async count_exports(){
        
        let sql =   "SELECT count(ae.id) as total FROM admin_exports ae INNER JOIN admin_user au ON au.id = ae.admin_user_id";

        let args = [];

        return await this.get_one(sql, args);
    }

    list_crons(type){
        
        let sql =   "SELECT * FROM admin_crons WHERE type = ?";

        let args = [type];

        return util.promisify( this.connection.query )
            .call( this.connection, sql, args );        
    }

    async get_gamemodes(){
        
        let sql =   "SELECT * FROM admin_gamemodes";

        let args = [];

        try{
            return await this.get_one(sql, args);
        } catch(exception){
            return Error("Error getting gamemodes!");                        
        }
    }
}

module.exports = AdminQuery;