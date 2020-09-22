<template>
  <div class="app-container">
    <el-table
      v-loading="listLoading"
      :data="list"
      element-loading-text="Loading"
      border
      fit
      highlight-current-row
      v-if="this.$store.state.user.role == 'admin'"
    >
      <el-table-column align="center" label="ID" width="95">
        <template slot-scope="scope">
          {{ scope.row.id }}
        </template>
      </el-table-column>
      <el-table-column label="Title">
        <template slot-scope="scope">
          {{ scope.row.title }}
        </template>
      </el-table-column>
      <el-table-column label="Email" align="center">
        <template slot-scope="scope">
          <span>{{ scope.row.email }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Role" width="110" align="center">
        <template slot-scope="scope">
          {{ scope.row.role }}
        </template>
      </el-table-column>
      <el-table-column label="Active" width="110" align="center">
        <template slot-scope="scope">
          {{ scope.row.active }}
        </template>
      </el-table-column>
      <el-table-column label="Created" width="200" align="center">
        <template slot-scope="scope">
          {{ scope.row.created | moment("D. M. YYYY, H:mm")  }}
        </template>
      </el-table-column>

      <el-table-column label="Action" width="200" align="center" v-if="this.$store.state.user.role == 'admin'">
        <template slot-scope="scope">
          <router-link
            :to="{name: 'editUser', params: {id: scope.row.id}}"
            tag="a"
            class="el-button el-button--primary el-button--small">
            <svg-icon icon-class="file-alt" />
            <span>Edit</span>
          </router-link>
        </template>
      </el-table-column>

    </el-table>
  </div>
</template>

<script>
import { getList } from '@/api/users'

export default {
  filters: {
    statusFilter(status) {
      const statusMap = {
        published: 'success',
        draft: 'gray',
        deleted: 'danger'
      }
      return statusMap[status]
    }
  },
  data() {
    return {
      list: null,
      listLoading: true
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    fetchData() {
      this.listLoading = true
      getList().then(response => {
        this.list = response.data
        this.listLoading = false
      })
    }
  }
}
</script>
