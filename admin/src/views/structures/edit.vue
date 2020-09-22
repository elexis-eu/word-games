<template>
  <div class="app-container">
    <el-form ref="form" :model="form" :rules="rules" label-width="240px" v-if="this.$store.state.user.role == 'admin'">
      <el-row>
          <el-col :span="12">
            <el-form-item label="ID" prop="id">
              <el-input v-model="form.id"/>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="Code" prop="code">
              <el-input v-model="form.name" />
              </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="Headword position" prop="headword_position">
              <el-select v-model="form.headword_position" placeholder="please select headword position">
                <el-option label="1" value="1" />
                <el-option label="2" value="2" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="Translation" prop="translation">
              <el-input v-model="form.text" />
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-form-item>
        <el-button v-if="!id" type="primary" @click="onSave">Create</el-button>
        <el-button v-if="id" type="primary" @click="onSave">Save</el-button>
        <el-button v-if="id" type="primary" @click="onDelete">Delete</el-button>
      </el-form-item>
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

import { getOne, save, deleteOne } from '@/api/structures'
import { Message } from 'element-ui'

export default {
  data() {
    return {
      form: {
        id: '',
        name: '',
        headword_position: '',
        text: '',
      },
      id : null,
      rules: {
          id: [
          { required: true, message: 'Please input static ID', trigger: 'blur' }],
          code: [
          { required: true, message: 'Please input code', trigger: 'blur' }],
          headword_position: [
          { required: true, message: 'Please select headword position', trigger: 'blur' }],
          translation: [
          { required: true, message: 'Please input translation', trigger: 'blur' }],
      },
    }
  },
  created() {

    if (this.$route.params.id) {
      this.id = this.$route.params.id

      getOne(this.id)
        .then(response => {
          this.form = response.data
          this.isChanged = false
        })
        .catch(error => {
          console.log(error)
          this.isChanged = true
        }).finally(() => {
          this.submitted = false
        })
    } else {
      this.override_password = true
    }
  },
  methods: {
    onSave(redirect = false) {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          const successMessage = 'Structure updated successfully!'

          this.submitted = true

          save(this.id, this.form)
            .then(response => {
              Message({
                message: successMessage,
                type: 'success',
                duration: 5 * 1000
              })

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
    onDelete(redirect = false) {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          const successMessage = 'Structure deleted successfully!'

          this.submitted = true

          deleteOne(this.id)
            .then(response => {
              Message({
                message: successMessage,
                type: 'success',
                duration: 5 * 1000
              })

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
    }
  }
}
</script>

<style scoped>
.line{
  text-align: center;
}
</style>

