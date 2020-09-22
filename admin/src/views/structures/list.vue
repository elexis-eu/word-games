<template>
  <div class="app-container">
    <el-table
      v-loading="listLoading"
      :data="list"
      element-loading-text="Loading"
      border
      fit
      highlight-current-row
    >
      <el-table-column align="center" label="ID" width="95">
        <template slot-scope="scope">
          {{ scope.row.id }}
        </template>
      </el-table-column>
      <el-table-column label="Code">
        <template slot-scope="scope">
          {{ scope.row.name }}
        </template>
      </el-table-column>
      <el-table-column label="Headword position" width="220" align="center">
        <template slot-scope="scope">
          <span>{{ scope.row.headword_position }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Translation" align="center">
        <template slot-scope="scope">
          {{ scope.row.text }}
        </template>
      </el-table-column>
      <el-table-column label="Action" width="200" align="center" v-if="this.$store.state.user.role == 'admin'">
        <template slot-scope="scope">
          <router-link
            :to="{name: 'editStructures', params: {id: scope.row.id}}"
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
import { getList } from '@/api/structures'

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
