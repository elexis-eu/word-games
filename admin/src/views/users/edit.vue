<template>
  <div class="app-container">
    <el-form ref="form" :model="form" :rules="rules" label-width="240px" v-if="this.$store.state.user.role == 'admin'">
      <el-row>
          <el-col :span="12">
            <el-form-item label="Active" prop="active">
              <el-switch v-model="form.active" :active-value="1" :inactive-value="0"/>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="Title" prop="title">
              <el-input v-model="form.title" />
              </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="Email" prop="email">
              <el-input v-model="form.email" />
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row v-if="id">
          <el-col :span="12">
            <el-form-item label="Override password">
              <el-switch v-model="override_password"/>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row v-if="override_password">
          <el-col :span="12">
            <el-form-item label="Password" prop="password">
              <el-input v-model="form.password" show-password/>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="Role" prop="role">
              <el-select v-model="form.role" placeholder="please select role">
                <el-option label="Admin" value="admin" />
                <el-option label="Editor" value="editor" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-form-item>
        <el-button v-if="!id" type="primary" @click="onCreate">Create</el-button>
        <el-button v-if="id" type="primary" @click="onSave">Save</el-button>
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

import { getOne, add, edit } from '@/api/users'
import { Message } from 'element-ui'

export default {
  data() {
    return {
      form: {
        active: 1,
        title: '',
        email: '',
        password: '',
        role : '',
      },
      id : null,
      override_password : false,
      rules: {
        title: [
          { required: true, message: 'Please input title', trigger: 'blur' },
          { min: 3, message: 'Length should be at least 3', trigger: 'blur' },
          { max: 100, message: 'Length should be at most 100', trigger: 'blur' }],
        email: [
          { required: true, message: 'Please input email address', trigger: 'blur' }
        ],
        password: [
          { required: true, message: 'Please input password', trigger: 'blur' }
        ],
        role: [
          { required: true, message: 'Please select role', trigger: 'change' }
        ],
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
    onCreate() {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          const successMessage = 'User updated successfully!'

          this.submitted = true

          if (this.form.password == null) {
            delete this.form.password
          }

          add(this.form)
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
    onSave(redirect = false) {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          const successMessage = 'User updated successfully!'

          this.submitted = true

          if (this.form.password == null) {
            delete this.form.password
          }

          edit(this.id, this.form)
            .then(response => {
              Message({
                message: successMessage,
                type: 'success',
                duration: 5 * 1000
              })

              setTimeout(() => this.$router.push({ name: 'listUser'}), 1000)

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

