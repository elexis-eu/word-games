<template>
  <div class="app-container">
    <el-form ref="form" :model="form" :rules="rules" label-width="240px" v-if="this.$store.state.user.role == 'admin'">
      <el-row>
          <el-col :span="12">
            <el-form-item label="Type" prop="type">
              <el-select v-model="form.type" placeholder="please select type" @change="changeImportType" :remote=true>
                <el-option
                      v-for="type in importTypesData"
                      :label="type.title"
                      :value="type.code"
                      :key="type.code"
                      />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="Delimiter" prop="delimiter">
              <el-input v-model="form.delimiter" maxlength="1"/>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="Format">
              <pre>{{ import_type_format }}</pre>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Description">
              <pre>{{ import_type_description }}</pre>
            </el-form-item>
          </el-col>
      </el-row>
      <el-form-item>
        <el-button v-if="!id" type="primary" @click="onSave">Create</el-button>
        <!--<el-button v-if="id" type="primary" @click="onSave">Save</el-button>-->
      </el-form-item>
      <el-row>
      </el-row>
      <el-row v-if="id">
          <el-col :span="12">
            <el-form-item label="File">
                   <el-upload
                      class="upload-demo"
                      drag
                      :action="upload_url"
                      :on-success="fileUploaded"
                      :before-upload="beforeUpload"
                      :headers="headers_upload"
                      :data="data_upload"
                      name="importcsv"
                      >
                  <i class="el-icon-upload"></i>
                  <div class="el-upload__text">Drop file here or <em>click to upload</em></div>
                  <!--<div class="el-upload__tip" slot="tip">jpg/png files with a size less than 500kb</div>-->
                </el-upload>
              </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row v-if="file_uploaded">
        <el-form-item>
          <el-button type="primary" @click="confirmUpload">Confirm upload</el-button>
          <!--<el-button v-if="id" type="primary" @click="onSave">Save</el-button>-->
        </el-form-item>
      </el-row>
      <!--
      <el-form-item label="Activity time">
        <el-col :span="11">
          <el-date-picker v-model="form.date1" type="date" placeholder="Pick a date" style="width: 100%;" />
        </el-col>
        <el-col :span="2" class="line">-</el-col>
        <el-col :span="11">
          <el-time-picker v-model="form.date2" type="fixed-time" placeholder="Pick a time" style="width: 100%;" />
        </el-col>
      </el-form-item>
      <el-form-item label="Instant delivery">
        <el-switch v-model="form.delivery" />
      </el-form-item>
      <el-form-item label="Activity type">
        <el-checkbox-group v-model="form.type">
          <el-checkbox label="Online activities" name="type" />
          <el-checkbox label="Promotion activities" name="type" />
          <el-checkbox label="Offline activities" name="type" />
          <el-checkbox label="Simple brand exposure" name="type" />
        </el-checkbox-group>
      </el-form-item>
      <el-form-item label="Resources">
        <el-radio-group v-model="form.resource">
          <el-radio label="Sponsor" />
          <el-radio label="Venue" />
        </el-radio-group>
      </el-form-item>
      <el-form-item label="Activity form">
        <el-input v-model="form.desc" type="textarea" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="onSubmit">Create</el-button>
        <el-button @click="onCancel">Cancel</el-button>
      </el-form-item>
      -->
    </el-form>
  </div>
</template>

<script>

import { save, importTypes } from '@/api/imports'
import { Message } from 'element-ui'
import { getToken } from '@/utils/auth'
import request from '@/utils/request'

export default {
  data() {
    return {
      form: {
        type: '',
        delimiter: ''
      },
      id: null,
      rules: {
        delimiter: [
          { required: true, message: 'Please input delimiter', trigger: 'blur' },
          { max: 1, message: 'Input only one character', trigger: 'blur' }],
        type: [
          { required: true, message: 'Please select type', trigger: 'blur' }]
      },
      headers_upload: {},
      data_upload: {},
      importTypesData: [],
      upload_url: '',
      file_uploaded: false,
      import_type_format: '',
      import_type_description: ''
    }
  },
  created() {
    this.headers_upload = { 'Authorization': 'Bearer ' + getToken() }
    this.upload_url = request.defaults.baseURL + '/api/v1/admin/imports/upload'
    importTypes().then(response => { this.importTypesData = response.data })
  },
  methods: {
    onSave(redirect = false) {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          const successMessage = 'Import in queue!'

          this.submitted = true

          if (this.form.password == null) {
            delete this.form.password
          }

          save(this.id, this.form)
            .then(response => {
              Message({
                message: successMessage,
                type: 'success',
                duration: 5 * 1000
              })

              this.id = response.data.id

              this.data_upload = { id: this.id }
            })
            .catch(error => {
              console.log(error)
              console.log(error.message)
              console.log(error.data)
            }).finally(() => {
              this.submitted = false
            })
        }
      })
    },
    onCancel() {
      this.$message({
        message: 'cancel!',
        type: 'warning'
      })
    },
    fileUploaded(response, file, fileList) {
      this.form.filename = file.name
      this.file_uploaded = true
    },
    beforeUpload(file) {
      this.data_upload.filename = file.name
    },
    confirmUpload() {
      setTimeout(() => this.$router.push({ name: 'listImports' }), 1000)
    },
    changeImportType(value) {
      this.importTypesData.forEach(function(item) {
        if (item.code === value) {
          this.import_type_format = item.format
          this.import_type_description = item.description
        }
      }.bind(this))
    }
  }
}
</script>

<style scoped>
.line{
  text-align: center;
}
</style>

