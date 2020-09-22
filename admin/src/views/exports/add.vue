<template>
  <div class="app-container">
    <el-form ref="form" :model="form" :rules="rules" label-width="240px" v-if="this.$store.state.user.role == 'admin'">
      <el-row>
          <el-col :span="12">
            <el-form-item label="Type" prop="type">
              <el-select v-model="form.type" placeholder="please select type">
                <el-option
                      v-for="type in exportTypesData"
                      :label="type.title"
                      :value="type.code"
                      :key="type.code"/>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="From date" prop="date_from">
                <el-date-picker
                  v-model="form.date_from"
                  type="date"
                  placeholder="Pick a day"
                  value-format="yyyy-MM-dd"
                  :picker-options="pickerOptions">
                </el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-row>
          <el-col :span="12">
            <el-form-item label="To date" prop="date_to">
                <el-date-picker
                  v-model="form.date_to"
                  type="date"
                  placeholder="Pick a day"
                  value-format="yyyy-MM-dd"
                  :picker-options="pickerOptions">
                </el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="12"></el-col>
      </el-row>
      <el-form-item>
        <el-button v-if="!id" type="primary" @click="onSave">Create</el-button>
        <!--<el-button v-if="id" type="primary" @click="onSave">Save</el-button>-->
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

import { save, exportTypes } from '@/api/exports'
import { Message } from 'element-ui'
import { getToken } from '@/utils/auth'

export default {
  data() {
    return {
      form: {
        type: '',
        date_from: '',
        date_to: '',
      },
      id : null,
      rules: {
          type: [
          { required: true, message: 'Please select type', trigger: 'blur' }],
      },
      exportTypesData: exportTypes(),
      pickerOptions: {
          disabledDate(time) {
            return time.getTime() > Date.now();
          },
          shortcuts: [{
            text: 'Today',
            onClick(picker) {
              picker.$emit('pick', new Date());
            }
          }, {
            text: 'Yesterday',
            onClick(picker) {
              const date = new Date();
              date.setTime(date.getTime() - 3600 * 1000 * 24);
              picker.$emit('pick', date);
            }
          }, {
            text: 'A week ago',
            onClick(picker) {
              const date = new Date();
              date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
              picker.$emit('pick', date);
            }
          }]
        },
    }
  },
  created() {
    exportTypes().then(response => { this.exportTypesData = response.data })
  },
  methods: {
    onSave(redirect = false) {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          const successMessage = 'Export in queue!'

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

              this.id = response.data.id;

              setTimeout(() => this.$router.push({ name: 'listExports'}), 1000)
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
  }
}
</script>

<style scoped>
.line{
  text-align: center;
}
</style>

